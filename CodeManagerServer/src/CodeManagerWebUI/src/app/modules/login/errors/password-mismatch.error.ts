import { BaseError } from './base.error';

export class PasswordMismatchError extends BaseError {
  constructor() {
    super('Different passwords provided!');
  }
}
