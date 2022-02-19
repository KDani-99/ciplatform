import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MemberDto } from '../../../../../../services/team/team.interface';

@Component({
  selector: 'team-member',
  templateUrl: './member.component.html',
  styleUrls: ['./member.component.scss'],
})
export class MemberComponent implements OnInit {
  @Input() member?: MemberDto;
  @Input() showKickButton: boolean = false;

  @Output() onKick: EventEmitter<MemberDto> = new EventEmitter<MemberDto>();
  @Output() onUpdateRole: EventEmitter<MemberDto> =
    new EventEmitter<MemberDto>();

  constructor() {}

  ngOnInit(): void {}

  getRoleFriendlyName(role: number) {
    switch (role) {
      case 0:
        return 'Read';
      case 1:
        return 'ReadWrite';
      case 2:
        return 'Admin';
    }
    return 'Unknown';
  }

  kick(): void {
    this.onKick.emit(this.member);
  }

  updateRole(): void {
    this.onUpdateRole.emit(this.member);
  }
}
