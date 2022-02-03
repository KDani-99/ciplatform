import { Component, Input, OnInit } from '@angular/core';
import { TeamDto } from '../../../../team/team.interface';

@Component({
  selector: 'teams-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  @Input() team: TeamDto = {
    name: '',
    description: '',
    id: NaN,
    image: '',
    isPublic: false,
    members: NaN,
    projects: NaN,
    owner: '',
  };

  constructor() {}

  ngOnInit(): void {}
}
