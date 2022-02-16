import { Component, Input, OnInit } from '@angular/core';
import { TeamService } from '../../../../services/team/team.service';
import {
  CreateTeamDto,
  TeamDto,
} from '../../../../services/team/team.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'app-teams',
  templateUrl: './teams.component.html',
  styleUrls: ['./teams.component.scss'],
})
export class TeamsComponent implements OnInit {
  @Input() teams: TeamDto[] = [];

  showCreateTeamsWindow: boolean = false;

  constructor(
    private readonly router: Router,
    private readonly teamService: TeamService,
  ) {}

  ngOnInit(): void {
    this.teamService.getTeams().subscribe((partialTeamDto: TeamDto[]) => {
      this.teams = partialTeamDto;
    });
  }

  toggleCreateProjectsWindow(show: boolean): void {
    this.showCreateTeamsWindow = show;
  }

  createTeam(createTeamDto: CreateTeamDto): void {
    this.teamService.createTeam(createTeamDto).subscribe((team: TeamDto) => {
      this.teams.push(team);
      this.showCreateTeamsWindow = false;
    });
  }

  joinTeam(id: number): void {
    this.teamService
      .joinTeam(id)
      .subscribe(() => this.router.navigate([`teams/${id}`]));
  }
}
