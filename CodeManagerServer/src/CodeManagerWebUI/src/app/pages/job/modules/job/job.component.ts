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
import { JobDataDto } from '../../../../services/run/run.interface';
import { State } from '../../../../services/run/run.interface';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.scss'],
})
export class JobComponent implements OnInit, OnDestroy {
  currentTemplate?: TemplateRef<any>;
  showWindow: boolean = false;
  @ViewChild('jobConsole') jobConsole?: TemplateRef<any>;

  jobDto?: JobDataDto;

  State = State;

  private runId: number = 0;
  private jobId: number = 0;

  log?: string[];

  constructor(
    private readonly route: ActivatedRoute,
    readonly runService: RunService,
    private readonly signalrService: SignalRService,
  ) {}

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
    this.signalrService.registerMethod(
      'ReceiveStepResultEvent',
      this.onReceiveStepResultEvent,
    );
    this.signalrService.subscribeToStepResultChannel(this.jobDto!.job.id);
  }

  onReceiveStepResultEvent(stepResultEvent: any): void {
    console.log('step result', stepResultEvent);
  }

  openConsoleWindow(stepId: number): void {
    this.runService
      .getStepFile(this.runId, this.jobId, stepId)
      .subscribe((content: string) => {
        this.log = content?.split('\n');
        this.toggleWindow(true, this.jobConsole);
      });
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
    }
  }
}
