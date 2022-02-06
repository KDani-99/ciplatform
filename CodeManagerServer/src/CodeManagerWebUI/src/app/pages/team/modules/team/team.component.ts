import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { TeamService } from '../../team.service';
import { TeamDto } from '../../team.interface';
import { ActivatedRoute } from '@angular/router';
import {
  CreateProjectDto,
  ProjectDto,
} from '../../../project/project.interface';
import { ProjectService } from '../../../project/project.service';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  currentTemplate?: TemplateRef<any>;

  @ViewChild('createProjectTemplate') createProjectTemplate?: TemplateRef<any>;
  @ViewChild('editTeamTemplate') editTeamTemplate?: TemplateRef<any>;

  team?: TeamDto;
  showWindow: boolean = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly teamService: TeamService,
    private readonly projectService: ProjectService,
  ) {}

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.teamService.getTeam(id).subscribe({
      next: (teamDto: TeamDto) => {
        this.team = teamDto;
      },
    });
  }

  openCreateProjectWindow(): void {
    this.toggleWindow(true, this.createProjectTemplate);
  }

  onCreateProject(createProjectDto: CreateProjectDto): void {
    createProjectDto.teamId = this.team!.id;
    this.projectService.createProject(createProjectDto).subscribe({
      next: (project: ProjectDto) => {
        // this.projects?.push(project);
        // TODO::
      },
    });
  }

  openEditWindow(): void {
    this.toggleWindow(true, this.editTeamTemplate);
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
    }
  }
}
