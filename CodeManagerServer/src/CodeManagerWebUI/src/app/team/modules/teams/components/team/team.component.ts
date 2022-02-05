import { Component, Input, OnInit } from '@angular/core';
import { TeamDto } from '../../../../team.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'teams-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  @Input() team?: TeamDto;

  constructor(private readonly router: Router) {}

  ngOnInit(): void {}

  async open(): Promise<void> {
    await this.router.navigate([`teams/${this.team?.id}`]);
  }
}
