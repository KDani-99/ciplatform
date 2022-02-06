import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MemberDto } from '../../../../team.interface';

@Component({
  selector: 'team-member',
  templateUrl: './member.component.html',
  styleUrls: ['./member.component.scss'],
})
export class MemberComponent implements OnInit {
  @Input() member?: MemberDto;
  @Input() showKickButton: boolean = false;

  @Output() onKick: EventEmitter<MemberDto> = new EventEmitter<MemberDto>();

  constructor() {}

  ngOnInit(): void {}

  kick(): void {
    this.onKick.emit(this.member);
  }
}
