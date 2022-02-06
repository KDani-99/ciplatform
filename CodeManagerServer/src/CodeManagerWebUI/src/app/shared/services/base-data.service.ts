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
    return this.httpClient.get<TResult[]>(url, this.httpOptions).pipe(
      tap((entities: TResult[]) =>
        BaseDataService.log(`Fetched ${entities.length} entities`),
      ),
      catchError(this.handleError<TResult[]>()),
    );
  }

  protected get<TResult>(url: string): Observable<TResult> {
    return this.httpClient.get<TResult>(url, this.httpOptions).pipe(
      tap((entity: any) =>
        BaseDataService.log(`Fetched entity with id ${entity?.id}`),
      ),
      catchError(this.handleError<TResult>()),
    );
  }

  protected create<T, TResult>(
    createEntityDto: T,
    url: string,
  ): Observable<TResult> {
    return this.httpClient
      .post<TResult>(url, createEntityDto, this.httpOptions)
      .pipe(
        tap((entity: any) =>
          BaseDataService.log(`Entity with id ${entity.id} has been created`),
        ),
        catchError(this.handleError<TResult>()),
      );
  }

  protected delete(id: number, url: string): Observable<void> {
    return this.httpClient.delete(url, this.httpOptions).pipe(
      tap((_) => BaseDataService.log(`Entity with id ${id} has been deleted`)),
      catchError(this.handleError<any>()),
    );
  }

  protected update<T>(id: number, entity: T, url: string): Observable<void> {
    return this.httpClient.put(url, entity, this.httpOptions).pipe(
      tap((_) => BaseDataService.log(`Entity with id ${id} has been updated`)),
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
