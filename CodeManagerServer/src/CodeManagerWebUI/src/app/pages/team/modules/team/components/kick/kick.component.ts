import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MemberDto } from '../../../../../../services/team/team.interface';

@Component({
  selector: 'team-kick',
  templateUrl: './kick.component.html',
  styleUrls: ['./kick.component.scss'],
})
export class KickComponent implements OnInit {
  @Input() member?: MemberDto;
  @Output() onCancel: EventEmitter<any> = new EventEmitter<any>();
  @Output() onKick: EventEmitter<number> = new EventEmitter<number>();

  constructor() {}

  ngOnInit(): void {}

  cancel(): void {
    this.onCancel.emit();
  }

  kick(): void {
    this.onKick.emit(this.member?.id ?? -1);
  }
}
