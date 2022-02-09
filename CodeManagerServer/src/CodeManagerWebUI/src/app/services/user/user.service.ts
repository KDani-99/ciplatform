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
    return this.httpClient.get<UserDto[]>(
      this.configService.getFullUrl('getUsers'),
      this.httpOptions,
    );
  }

  deleteUser(id: number): Observable<any> {
    const url = `${this.configService.getFullUrl('deleteUser')}/${id}`;
    return this.httpClient.delete(url, this.httpOptions);
  }

  updateUser(id: number, updateUserDto: UpdateUserDto): Observable<any> {
    const url = `${this.configService.getFullUrl('updateUser')}/${id}`;
    return this.httpClient.put<UpdateUserDto>(
      url,
      updateUserDto,
      this.httpOptions,
    );
  }
}
