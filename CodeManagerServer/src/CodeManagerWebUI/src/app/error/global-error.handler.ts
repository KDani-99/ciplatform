import { ErrorHandler, Inject, Injectable, Injector } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BaseError } from '../auth/modules/login/errors/base.error';

@Injectable()
export class GlobalErrorHandler extends ErrorHandler {
  constructor(@Inject(Injector) private readonly injector: Injector) {
    super();
  }

  handleError(error: any) {
    if (error.promise && error.rejection) {
      error = error.rejection;
    }

    if (error.type === BaseError.name) {
    } else {
      super.handleError(error);
    }
  }
}
