import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RunService } from '../../../../services/run/run.service';
import {
  JobDto,
  RunDataDto,
  State,
} from '../../../../services/run/run.interface';

@Component({
  selector: 'app-run',
  templateUrl: './run.component.html',
  styleUrls: ['./run.component.scss'],
})
export class RunComponent implements OnInit {
  runData?: RunDataDto;

  State = State;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly runService: RunService,
  ) {}

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.runService.getRun(id).subscribe((runData: RunDataDto) => {
      this.runData = runData;
    });
  }

  async openJob(id: number): Promise<void> {
    await this.router.navigate([`runs/${this.runData?.run.id}/jobs/${id}`]);
  }

  getFinishedTime(job: JobDto): string {
    return job.state === State.RUNNING
      ? '-'
      : new Date(job.finishedDateTime).toLocaleString();
  }

  getStartedTime(job: JobDto): string {
    return new Date(job.startedDateTime).toLocaleString();
  }
}
