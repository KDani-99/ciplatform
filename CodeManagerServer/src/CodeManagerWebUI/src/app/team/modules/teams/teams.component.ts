import { Component, Input, OnInit } from '@angular/core';
import { TeamService } from '../../team.service';
import { CreateTeamDto, TeamDto } from '../../team.interface';

@Component({
  selector: 'app-teams',
  templateUrl: './teams.component.html',
  styleUrls: ['./teams.component.scss'],
})
export class TeamsComponent implements OnInit {
  @Input() teams?: TeamDto[];

  showCreateTeamsWindow: boolean = false;

  constructor(private readonly teamService: TeamService) {}

  ngOnInit(): void {
    this.teamService.getTeams().subscribe({
      next: (partialTeamDto: TeamDto[]) => {
        this.teams = partialTeamDto;
      },
    });
  }

  toggleCreateProjectsWindow(show: boolean): void {
    this.showCreateTeamsWindow = show;
  }

  async createTeam(createTeamDto: CreateTeamDto): Promise<void> {
    this.teamService.createTeam(createTeamDto).subscribe({
      next: (team: TeamDto) => {
        this.teams?.push(team);
        this.showCreateTeamsWindow = false;
      },
    });
  }
}
