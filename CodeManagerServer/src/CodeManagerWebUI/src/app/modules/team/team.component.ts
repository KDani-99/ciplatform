import { Component, OnInit } from '@angular/core';
import { TeamService } from '../../team/team.service';
import { TeamDto } from '../../team/team.interface';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  team?: TeamDto;
  showEditWindow: boolean = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly teamService: TeamService,
  ) {}

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.teamService.getTeam(id).subscribe({
      next: (teamDto: TeamDto) => {
        this.team = teamDto;
      },
    });
  }

  loadTeam(): void {}

  toggleEditWindow(show: boolean): void {
    this.showEditWindow = show;
  }
}
