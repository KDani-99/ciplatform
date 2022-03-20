import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../../../services/project/project.service';
import {
  CreateProjectDto,
  ProjectDto,
} from '../../../../services/project/project.interface';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';

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
