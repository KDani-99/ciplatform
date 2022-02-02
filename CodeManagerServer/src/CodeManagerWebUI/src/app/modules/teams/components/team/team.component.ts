import { Component, Input, OnInit } from '@angular/core';
import { Team } from './team.interface';

@Component({
  selector: 'teams-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  @Input() team: Team = {
    name: '',
    description: '',
    id: NaN,
    image: '',
    isPublic: false,
    members: NaN,
    projects: NaN,
  };

  constructor() {}

  ngOnInit(): void {}
}
