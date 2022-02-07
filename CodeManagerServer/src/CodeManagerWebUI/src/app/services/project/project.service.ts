import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from '../../config/config.service';
import {
  CreateProjectDto,
  ProjectDataDto,
  ProjectDto,
} from './project.interface';
import { BaseDataService } from '../../shared/services/base-data.service';

@Injectable({ providedIn: 'root' })
export class ProjectService extends BaseDataService {
  constructor(
    httpClient: HttpClient,
    private readonly configService: ConfigService,
  ) {
    super(httpClient);
  }

  getProjects(): Observable<ProjectDto[]> {
    return super.getAll<ProjectDto>(this.configService.getFullUrl('projects'));
  }

  getProject(id: number): Observable<ProjectDataDto> {
    const url = `${this.configService.getFullUrl('projects')}/${id}`;
    return super.get<ProjectDataDto>(url);
  }

  createProject(createProjectDto: CreateProjectDto): Observable<ProjectDto> {
    return super.create<CreateProjectDto, ProjectDto>(
      createProjectDto,
      this.configService.getFullUrl('projects'),
    );
  }

  deleteProject(id: number): Observable<void> {
    return super.delete(
      id,
      `${this.configService.getFullUrl('projects')}/${id}`,
    );
  }

  updateProject(project: CreateProjectDto, id: number): Observable<void> {
    return super.update(
      id,
      project,
      `${this.configService.getFullUrl('projects')}/${id}`,
    );
  }
}
