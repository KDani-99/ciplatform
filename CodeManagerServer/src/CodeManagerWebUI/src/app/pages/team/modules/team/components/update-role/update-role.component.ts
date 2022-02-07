import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  availablePermissions,
  MemberDto,
  Permission,
  UpdateRoleDto,
} from '../../../../../../services/team/team.interface';

@Component({
  selector: 'team-update-role',
  templateUrl: './update-role.component.html',
  styleUrls: ['./update-role.component.scss'],
})
export class UpdateRoleComponent implements OnInit {
  @Input() member!: MemberDto;
  @Input() permissions!: Permission[];
  @Output() onCancel: EventEmitter<any> = new EventEmitter<any>();
  @Output() onUpdate: EventEmitter<UpdateRoleDto> =
    new EventEmitter<UpdateRoleDto>();

  value: string = '0';

  constructor() {
    this.permissions = availablePermissions;
  }

  ngOnInit(): void {}

  cancel(): void {
    this.onCancel.emit();
  }

  update(): void {
    this.onUpdate.emit({
      role: parseInt(this.value),
      userId: this.member?.id ?? -1,
    });
  }
}
