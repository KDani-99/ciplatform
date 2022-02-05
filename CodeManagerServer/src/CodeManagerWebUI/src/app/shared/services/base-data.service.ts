import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

interface Entity {
  id: number;
}

export abstract class BaseDataService<T, TResult extends Entity> {
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
  };

  protected constructor(private readonly httpClient: HttpClient) {}

  protected getAll(url: string): Observable<TResult[]> {
    return this.httpClient.get<TResult[]>(url, this.httpOptions).pipe(
      tap((entities: TResult[]) =>
        BaseDataService.log(`Fetched ${entities.length} entities`),
      ),
      catchError(this.handleError<TResult[]>()),
    );
  }

  protected get(url: string): Observable<TResult> {
    return this.httpClient.get<TResult>(url, this.httpOptions).pipe(
      tap((entity: TResult) =>
        BaseDataService.log(`Fetched entity with id ${entity.id}`),
      ),
      catchError(this.handleError<TResult>()),
    );
  }

  protected create(createEntityDto: T, url: string): Observable<TResult> {
    return this.httpClient
      .post<TResult>(url, createEntityDto, this.httpOptions)
      .pipe(
        tap((entity: TResult) =>
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

  protected update(id: number, entity: T, url: string): Observable<void> {
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
