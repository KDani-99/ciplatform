import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RunService } from '../../../../services/run/run.service';
import { RunDataDto, State } from '../../../../services/run/run.interface';

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
    readonly runService: RunService,
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
}
