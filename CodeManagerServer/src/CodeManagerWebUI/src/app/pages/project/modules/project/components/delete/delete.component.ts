import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'project-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.scss'],
})
export class DeleteComponent implements OnInit {
  @Input() projectName?: string;
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
