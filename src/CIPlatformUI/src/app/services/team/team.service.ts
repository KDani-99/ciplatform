import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import {
  CreateTeamDto,
  TeamDataDto,
  TeamDto,
  UpdateRoleDto,
} from './team.interface';
import { ConfigService } from '../../config/config.service';

@Injectable({ providedIn: 'root' })
export class TeamService {
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
  };

  constructor(
    private readonly httpClient: HttpClient,
    private readonly configService: ConfigService,
  ) {}

  getTeams(): Observable<TeamDto[]> {
    return this.httpClient.get<TeamDto[]>(
      this.configService.getFullUrl('teams'),
      this.httpOptions,
    );
  }

  getTeam(id: number): Observable<TeamDataDto> {
    const url = `${this.configService.getFullUrl('teams')}/${id}`;
    return this.httpClient.get<TeamDataDto>(url, this.httpOptions);
  }

  createTeam(createTeamDto: CreateTeamDto): Observable<TeamDto> {
    return this.httpClient.post<TeamDto>(
      this.configService.getFullUrl('teams'),
      createTeamDto,
      this.httpOptions,
    );
  }

  joinTeam(id: number): Observable<any> {
    const url = `${this.configService
      .getFullUrl('joinTeam')
      .replace('{0}', id.toString())}`;

    return this.httpClient.post(url, {}, this.httpOptions);
  }

  kickMember(id: number, memberId: number): Observable<any> {
    const url = `${this.configService
      .getFullUrl('kickMember')
      .replace('{0}', id.toString())}`;

    return this.httpClient.post(url, { memberId }, this.httpOptions);
  }

  addMember(id: number, username: string): Observable<any> {
    const url = `${this.configService
      .getFullUrl('addMember')
      .replace('{0}', id.toString())}`;

    return this.httpClient.post(url, { username }, this.httpOptions);
  }

  updateMemberRole(id: number, updateRoleDto: UpdateRoleDto): Observable<any> {
    const url = `${this.configService
      .getFullUrl('updateRole')
      .replace('{0}', id.toString())}`;

    return this.httpClient.post(url, updateRoleDto, this.httpOptions);
  }

  deleteTeam(id: number): Observable<any> {
    const url = `${this.configService.getFullUrl('teams')}/${id}`;

    return this.httpClient.delete(url, this.httpOptions);
  }

  updateTeam(team: CreateTeamDto, id: number): Observable<any> {
    const url = `${this.configService.getFullUrl('teams')}/${id}`;
    return this.httpClient.put(url, team, this.httpOptions);
  }
}
