import {
  Component,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { SignalRService } from '../../../../services/signalr/signalr.service';
import { ActivatedRoute } from '@angular/router';
import { RunService } from '../../../../services/run/run.service';
import { JobDataDto, State } from '../../../../services/run/run.interface';
import { ProcessedStepResult } from '../../../../services/run/event-result.interface';
import { BaseRunComponent } from '../../../../shared/components/base-run.component';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.scss'],
})
export class JobComponent
  extends BaseRunComponent
  implements OnInit, OnDestroy
{
  currentTemplate?: TemplateRef<any>;
  showWindow: boolean = false;
  @ViewChild('jobConsole') jobConsole?: TemplateRef<any>;

  jobDto?: JobDataDto;

  State = State;

  private runId: number = 0;
  private jobId: number = 0;

  log?: string[];
  private stepId?: number;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly signalrService: SignalRService,
    runService: RunService,
  ) {
    super(runService);
  }

  ngOnInit(): void {
    this.runId = parseInt(this.route.snapshot.paramMap.get('runId')!, 10);
    this.jobId = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.runService
      .getJob(this.runId, this.jobId)
      .subscribe((jobDto: JobDataDto) => {
        this.jobDto = jobDto;
        this.listen();
      });
  }

  ngOnDestroy(): void {
    this.signalrService.unSubscribeFromStepResultChannel(this.jobDto!.job.id);
  }

  listen(): void {
    this.signalrService.registerMethod('ReceiveStepResultEvent', (event: any) =>
      this.onReceiveStepResultEvent(event),
    );
    this.signalrService.subscribeToStepResultChannel(this.jobDto!.job.id);
  }

  onReceiveStepResultEvent(stepResultEvent: ProcessedStepResult): void {
    const selectedStep = this.jobDto?.steps.find(
      (step) => step.id === stepResultEvent.stepId,
    );
    super.onReceiveResultEvent(selectedStep, stepResultEvent);
  }

  openConsoleWindow(stepId: number): void {
    const step = this.jobDto?.steps?.find((step) => step.id === stepId);
    if (step?.state === State.NOT_RUN || step?.state === State.SKIPPED) {
      return;
    }

    this.stepId = step!.id;

    this.runService
      .getStepFile(this.runId, this.jobId, stepId)
      .subscribe((content: string) => {
        this.log = content?.split('\n');
        this.toggleWindow(true, this.jobConsole);
      });
    this.signalrService.subscribeToLogsChannel(stepId);
    this.signalrService.registerMethod('ReceiveLogs', (content: any) =>
      this.onReceiveLogs(content),
    );
  }

  onReceiveLogs(content: string) {
    if (content === '') {
      return;
    }
    this.log?.push(content);
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      if (this.currentTemplate === this.jobConsole) {
        this.signalrService.unSubscribeFromLogsChannel(this.stepId!);
      }
      this.currentTemplate = undefined;
    }
  }
}
