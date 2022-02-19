import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpStatusCode,
} from '@angular/common/http';
import { catchError, concatMap } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';
import { ErrorService } from '../services/error/error.service';

@Injectable()
export class HttpGlobalInterceptor implements HttpInterceptor {
  private retrying: boolean = false;

  constructor(
    private readonly authService: AuthService,
    private readonly errorService: ErrorService,
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (
          error.status === HttpStatusCode.Unauthorized &&
          error.headers.has('Token-Expired')
        ) {
          return this.refreshToken(request).pipe(
            concatMap((authReq) => next.handle(authReq)),
            catchError((nestedError: HttpErrorResponse) => {
              this.handleError(nestedError.error);
              return throwError(() => nestedError);
            }),
          );
        }

        this.handleError(error.error);

        return throwError(() => error);
      }),
    );
  }

  refreshToken(request: HttpRequest<any>): Observable<HttpRequest<any>> {
    if (!this.retrying) {
      return new Observable<HttpRequest<any>>((observer) => {
        this.retrying = true;
        this.authService
          .refreshToken()
          .then((accessToken: string) => {
            observer.next(
              request.clone({
                headers: request.headers.set(
                  'Authorization',
                  `Bearer ${accessToken}`,
                ),
              }),
            );
          })
          .catch((error: any) => {
            observer.error(error);
          })
          .finally(() => (this.retrying = false));
      });
    }
    return new Observable<HttpRequest<any>>((observer) => observer.next());
  }

  private handleError(error: any): void {
    if (error?.errors) {
      // multiple errors
      this.errorService.show(error.errors);
    } else if (error?.error) {
      // single error
      this.errorService.show(error?.error);
    }
  }
}
