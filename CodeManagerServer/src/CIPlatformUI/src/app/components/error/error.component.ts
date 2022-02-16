import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.scss'],
})
export class ErrorComponent implements OnInit {
  @Input() error?: Record<string, string[]> | string | null;
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }

  isSimpleError(): boolean {
    return typeof this.error === 'string';
  }

  getRecord(): Record<string, string> {
    if (!this.isSimpleError())
      return this.error as unknown as Record<string, string>;
    return {};
  }
}
