import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../project.service';
import { CreateProjectDto, ProjectDto } from '../../project.interface';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss'],
})
export class ProjectsComponent implements OnInit {
  constructor(private readonly projectService: ProjectService) {}

  projects?: ProjectDto[] = [];

  ngOnInit(): void {
    this.projectService.getProjects().subscribe({
      next: (projects: ProjectDto[]) => {
        this.projects = projects;
      },
    });
  }
}
