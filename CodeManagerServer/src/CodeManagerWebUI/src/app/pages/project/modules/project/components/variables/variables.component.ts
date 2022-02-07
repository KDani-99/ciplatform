import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'project-variables',
  templateUrl: './variables.component.html',
  styleUrls: ['./variables.component.scss'],
})
export class VariablesComponent implements OnInit {
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }
}
