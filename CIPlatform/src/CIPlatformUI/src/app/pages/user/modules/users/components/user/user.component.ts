import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { UserDto } from '../../../../../../services/user/user.interface';

@Component({
  selector: 'users-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
})
export class UserComponent implements OnInit {
  @Input() user?: UserDto;

  @Output() onEdit: EventEmitter<UserDto> = new EventEmitter<UserDto>();
  @Output() onDelete: EventEmitter<UserDto> = new EventEmitter<UserDto>();

  constructor() {}

  ngOnInit(): void {}

  delete(): void {
    this.onDelete.emit(this.user);
  }

  edit(): void {
    this.onEdit.emit(this.user);
  }
}
