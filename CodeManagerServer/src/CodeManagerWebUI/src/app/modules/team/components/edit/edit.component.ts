import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'team-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditComponent implements OnInit {
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }
}
