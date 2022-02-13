import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RunService } from '../../../../services/run/run.service';
import { RunDataDto, State } from '../../../../services/run/run.interface';
import { SignalRService } from '../../../../services/signalr/signalr.service';
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
    this.signalrService.unSubscribeFromJobResultChannel(this.runData!.run.id);
  }

  listen(): void {
    this.signalrService.registerMethod('ReceiveJobResultEvent', (event: any) =>
      this.onReceiveJobResultEvent(event),
    );
    this.signalrService.subscribeToJobResultChannel(this.runData!.run.id);
  }

  onReceiveJobResultEvent(jobResultEvent: ProcessedJobResult): void {
    const selectedJob = this.runData?.jobs.find(
      (job) => job.id === jobResultEvent.jobId,
    );
    /*
    if (!selectedJob) {
      return;
    }

    selectedJob.state = jobResultEvent.state;
    selectedJob.finishedDateTime = this.runService.getTime(
      jobResultEvent.dateTime,
    );*/
    super.onReceiveResultEvent(selectedJob, jobResultEvent);
  }

  async openJob(id: number): Promise<void> {
    await this.router.navigate([`runs/${this.runData?.run.id}/jobs/${id}`]);
  }
}
