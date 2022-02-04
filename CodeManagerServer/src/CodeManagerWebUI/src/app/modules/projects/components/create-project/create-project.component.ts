import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'projects-create-project',
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.scss'],
})
export class CreateProjectComponent implements OnInit {
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();

  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }
}
