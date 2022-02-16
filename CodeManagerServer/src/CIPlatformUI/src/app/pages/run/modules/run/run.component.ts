import {
  Component,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RunService } from '../../../../services/run/run.service';
import { RunDataDto, State } from '../../../../services/run/run.interface';
import {
  ResultsChannel,
  SignalRService,
} from '../../../../services/signalr/signalr.service';
import { ProcessedJobResult } from '../../../../services/run/event-result.interface';
import { BaseRunComponent } from '../../../../shared/components/base-run.component';

@Component({
  selector: 'app-run',
  templateUrl: './run.component.html',
  styleUrls: ['./run.component.scss'],
})
export class RunComponent
  extends BaseRunComponent
  implements OnInit, OnDestroy
{
  @ViewChild('deleteRunTemplate') deleteRunTemplate?: TemplateRef<any>;

  currentTemplate?: TemplateRef<any>;
  showWindow: boolean = false;
  runData?: RunDataDto;

  State = State;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly signalrService: SignalRService,
    runService: RunService,
  ) {
    super(runService);
  }

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.runService.getRun(id).subscribe((runData: RunDataDto) => {
      this.runData = runData;
      this.listen();
    });
  }

  ngOnDestroy(): void {
    this.signalrService.subscribeToResultsChannel(
      ResultsChannel.RUN,
      this.runData!.run.id,
    );
  }

  listen(): void {
    this.signalrService.registerMethod('ReceiveJobResultEvent', (event: any) =>
      this.onReceiveJobResultEvent(event),
    ); // TODO <- remove
    this.signalrService.subscribeToResultsChannel(
      ResultsChannel.RUN,
      this.runData!.run.id,
    );
  }

  onReceiveJobResultEvent(jobResultEvent: ProcessedJobResult): void {
    const selectedJob = this.runData?.jobs.find(
      (job) => job.id === jobResultEvent.jobId,
    );

    super.onReceiveResultEvent(selectedJob, jobResultEvent);
  }

  onDelete(): void {
    this.runService.deleteRun(this.runData!.run!.id).subscribe(() => {
      this.router.navigate(['teams']);
    });
  }

  openDeleteWindow(): void {
    this.toggleWindow(true, this.deleteRunTemplate);
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
    }
  }

  async openJob(id: number): Promise<void> {
    await this.router.navigate([`runs/${this.runData?.run.id}/jobs/${id}`]);
  }
}
