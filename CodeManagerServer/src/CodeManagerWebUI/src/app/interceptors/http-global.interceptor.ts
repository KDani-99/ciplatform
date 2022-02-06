import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpStatusCode,
} from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { catchError, concatMap } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';

@Injectable()
export class HttpGlobalInterceptor implements HttpInterceptor {
  private retrying: boolean = false;

  constructor(
    private readonly toastrService: ToastrService,
    private readonly authService: AuthService,
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === HttpStatusCode.Unauthorized) {
          return this.refreshToken(request).pipe(
            concatMap((authReq) => next.handle(authReq)),
          );
        }
        this.toastrService.error(
          'An unexpected HTTP error has occured',
          'Error',
          {
            closeButton: true,
            disableTimeOut: true,
          },
        );
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
          .catch((error: any) => observer.error(error))
          .finally(() => (this.retrying = false));
      });
    }
    return new Observable<HttpRequest<any>>((observer) => observer.next());
  }
}
