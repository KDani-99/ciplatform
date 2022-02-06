import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigService } from '../../config/config.service';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { UpdateUserDto, UserDto } from './user.interface';

@Injectable({ providedIn: 'root' })
export class UserService {
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
  };

  constructor(
    private readonly httpClient: HttpClient,
    private readonly configService: ConfigService,
  ) {}

  getUsers(): Observable<UserDto[]> {
    return this.httpClient
      .get<UserDto[]>(
        this.configService.getFullUrl('getUsers'),
        this.httpOptions,
      )
      .pipe(
        tap((users) => UserService.log(`Fetched ${users.length} entities`)),
        catchError(this.handleError<any>()),
      );
  }

  deleteUser(id: number): Observable<void> {
    const url = `${this.configService.getFullUrl('deleteUser')}/${id}`;
    return this.httpClient.delete(url, this.httpOptions).pipe(
      tap((_) => UserService.log(`Deleted user with id ${id}`)),
      catchError(this.handleError<any>()),
    );
  }

  updateUser(id: number, updateUserDto: UpdateUserDto): Observable<UserDto> {
    const url = `${this.configService.getFullUrl('updateUser')}/${id}`;
    return this.httpClient
      .put<UserDto>(url, updateUserDto, this.httpOptions)
      .pipe(
        tap((_) => UserService.log(`Updated user with id ${id}`)),
        catchError(this.handleError<any>()),
      );
  }

  resetPassword(id: number): Observable<void> {
    const url = `${this.configService.getFullUrl('resetPassword')}/${id}`;
    return this.httpClient.post(url, this.httpOptions).pipe(
      tap((_) => UserService.log(`Updated user with id ${id}`)),
      catchError(this.handleError<any>()),
    );
  }

  protected handleError<T>(result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);
      return of(result as T);
    };
  }

  private static log(message: string) {
    console.error(message);
  }
}
