import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

export abstract class BaseDataService {
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
  };

  protected constructor(private readonly httpClient: HttpClient) {}

  protected getAll<TResult>(url: string): Observable<TResult[]> {
    return this.httpClient.get<TResult[]>(url, this.httpOptions);
  }

  protected get<TResult>(url: string): Observable<TResult> {
    return this.httpClient.get<TResult>(url, this.httpOptions);
  }

  protected create<T, TResult>(
    createEntityDto: T,
    url: string,
  ): Observable<TResult> {
    return this.httpClient.post<TResult>(
      url,
      createEntityDto,
      this.httpOptions,
    );
  }

  protected delete(id: number, url: string): Observable<any> {
    return this.httpClient.delete(url, this.httpOptions);
  }

  protected update<T>(id: number, entity: T, url: string): Observable<any> {
    return this.httpClient.put(url, entity, this.httpOptions);
  }
}
