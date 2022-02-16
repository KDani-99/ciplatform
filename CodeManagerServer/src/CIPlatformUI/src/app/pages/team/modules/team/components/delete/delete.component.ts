import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'team-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.scss'],
})
export class DeleteComponent implements OnInit {
  @Input() teamName?: string;
  @Output() onCancel: EventEmitter<any> = new EventEmitter<any>();
  @Output() onDelete: EventEmitter<any> = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  cancel(): void {
    this.onCancel.emit();
  }

  delete(): void {
    this.onDelete.emit();
  }
}
