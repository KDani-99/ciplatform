import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
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

/*
@Injectable({ providedIn: 'root' })
export class ProjectService {
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  };

  constructor(
    private readonly httpClient: HttpClient,
    private readonly configService: ConfigService,
  ) {}

  getProjects(): Observable<ProjectDto[]> {
    return this.httpClient
      .get<ProjectDto[]>(this.configService.getFullUrl('getProjects'))
      .pipe(
        tap((projects) =>
          ProjectService.log(`Fetched ${projects.length} projects`),
        ),
        catchError(this.handleError<ProjectDto[]>()),
      );
  }

  getProject(id: number): Observable<ProjectDto> {
    const url = `${this.configService.getFullUrl('getProjects')}/${id}`;
    return this.httpClient.get<ProjectDto>(url).pipe(
      tap((project) =>
        ProjectService.log(`Fetched project with id ${project.id}`),
      ),
      catchError(this.handleError<ProjectDto>()),
    );
  }

  createProject(createProjectDto: CreateProjectDto): Observable<ProjectDto> {
    return this.httpClient
      .post<CreateProjectDto>(
        this.configService.getFullUrl('createProject'),
        createProjectDto,
        this.httpOptions,
      )
      .pipe(
        tap((team: any) =>
          ProjectService.log(`Project with id ${team.id} has been created`),
        ),
        catchError(this.handleError<ProjectDto>()),
      );
  }

  deleteProject(id: number): Observable<void> {
    const url = `${this.configService.getFullUrl('deleteTeam')}/${id}`;

    return this.httpClient.delete(url, this.httpOptions).pipe(
      tap((_) => ProjectService.log(`Team with id ${id} has been deleted`)),
      catchError(this.handleError<any>()),
    );
  }

  updateProject(project: CreateProjectDto, id: number): Observable<void> {
    const url = `${this.configService.getFullUrl('updateProject')}/${id}`;
    return this.httpClient.put(url, project, this.httpOptions).pipe(
      tap((_) =>
        ProjectService.log(`Project with id ${project} has been updated`),
      ),
      catchError(this.handleError<any>()),
    );
  }

  private handleError<T>(result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);
      return of(result as T);
    };
  }

  private static log(message: string) {
    console.error(message);
  }
}
*/
