import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'teams-create-team',
  templateUrl: './create-team.component.html',
  styleUrls: ['./create-team.component.scss'],
})
export class CreateTeamComponent implements OnInit {
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }
}
