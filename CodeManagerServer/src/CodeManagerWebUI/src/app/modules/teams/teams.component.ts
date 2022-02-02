import { Component, Input, OnInit } from '@angular/core';
import { Team } from './components/team/team.interface';
import { TeamService } from '../../team/team.service';
import { TeamDto } from '../../team/team.interface';

@Component({
  selector: 'app-teams',
  templateUrl: './teams.component.html',
  styleUrls: ['./teams.component.scss'],
})
export class TeamsComponent implements OnInit {
  @Input() teams: TeamDto[] = [
    {
      name: 'Test #1',
      description: 'bbb',
      id: NaN,
      image:
        'https://thequantuminsider.com/wp-content/uploads/2020/08/aws-logo.png',
      isPublic: false,
      members: 0,
      projects: 0,
      owner: '',
    },
    {
      name: 'Test #2',
      description: 'aaa',
      id: NaN,
      image: '',
      isPublic: true,
      members: 10,
      projects: 10,
      owner: '',
    },
  ];

  constructor(private readonly teamService: TeamService) {}

  ngOnInit(): void {
    this.teamService.getTeams().subscribe({
      next: (partialTeamDto: TeamDto[]) => {
        this.teams = partialTeamDto;
      },
    });
  }
}
