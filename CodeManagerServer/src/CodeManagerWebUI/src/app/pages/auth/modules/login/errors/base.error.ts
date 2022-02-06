export abstract class BaseError extends Error {
  private readonly type: string;
  protected constructor(message?: string) {
    super(message);
    this.type = 'BaseError';
    Object.setPrototypeOf(this, BaseError.prototype);
  }
}
