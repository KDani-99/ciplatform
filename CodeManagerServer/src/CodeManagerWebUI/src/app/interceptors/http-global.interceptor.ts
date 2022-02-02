import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

@Injectable()
export class HttpGlobalInterceptor implements HttpInterceptor {
  constructor(private readonly toastrService: ToastrService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        this.toastrService.error('An unexpected error has occured', 'Error', {
          closeButton: true,
          disableTimeOut: true,
        });
        return throwError(error);
      }),
    );
  }
}
