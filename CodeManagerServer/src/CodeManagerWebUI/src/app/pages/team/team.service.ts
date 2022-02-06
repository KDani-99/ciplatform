import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { CreateTeamDto, TeamDataDto, TeamDto } from './team.interface';
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
    return this.httpClient
      .get<TeamDto[]>(this.configService.getFullUrl('teams'), this.httpOptions)
      .pipe(
        tap((teams) => TeamService.log(`Fetched ${teams.length} teams`)),
        catchError(this.handleError<TeamDto[]>()),
      );
  }

  getTeam(id: number): Observable<TeamDataDto> {
    const url = `${this.configService.getFullUrl('teams')}/${id}`;
    return this.httpClient.get<TeamDataDto>(url, this.httpOptions).pipe(
      tap((team) => TeamService.log(`Fetched team with id ${team.id}`)),
      catchError(this.handleError<TeamDataDto>()),
    );
  }

  createTeam(createTeamDto: CreateTeamDto): Observable<TeamDto> {
    return this.httpClient
      .post<TeamDto>(
        this.configService.getFullUrl('teams'),
        createTeamDto,
        this.httpOptions,
      )
      .pipe(
        tap((team: TeamDto) =>
          TeamService.log(`Team with id ${team.id} has been created`),
        ),
        catchError(this.handleError<TeamDto>()),
      );
  }

  joinTeam(id: number): Observable<void> {
    const url = `${this.configService
      .getFullUrl('joinTeam')
      .replace('{0}', id.toString())}`;

    return this.httpClient.post(url, {}, this.httpOptions).pipe(
      tap((_) => TeamService.log(`You have entered team ${id}`)),
      catchError(this.handleError<any>()),
    );
  }

  kickmember(id: number, memberId: number): Observable<void> {
    const url = `${this.configService
      .getFullUrl('kickMember')
      .replace('{0}', id.toString())}`;

    return this.httpClient.post(url, { memberId }, this.httpOptions).pipe(
      tap((_) => TeamService.log(`You have entered team ${id}`)),
      catchError(this.handleError<any>()),
    );
  }

  deleteTeam(id: number): Observable<void> {
    const url = `${this.configService.getFullUrl('teams')}/${id}`;

    return this.httpClient.delete(url, this.httpOptions).pipe(
      tap((_) => TeamService.log(`Team with id ${id} has been deleted`)),
      catchError(this.handleError<any>()),
    );
  }

  updateTeam(team: CreateTeamDto, id: number): Observable<void> {
    const url = `${this.configService.getFullUrl('teams')}/${id}`;
    return this.httpClient.put(url, team, this.httpOptions).pipe(
      tap((_) => TeamService.log(`Team with id ${team} has been updated`)),
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
