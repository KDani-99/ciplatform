import { ErrorHandler, Inject, Injectable, Injector } from '@angular/core';

@Injectable()
export class GlobalErrorHandler extends ErrorHandler {
  constructor(@Inject(Injector) private readonly injector: Injector) {
    super();
  }

  handleError(error: any) {
    if (error.promise && error.rejection) {
      error = error.rejection;
    }
    super.handleError(error);
  }
}
