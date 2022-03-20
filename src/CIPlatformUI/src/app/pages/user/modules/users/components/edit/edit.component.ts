import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  UpdateUserDto,
  UserDto,
} from '../../../../../../services/user/user.interface';
import { CheckboxComponent } from '../../../../../../shared/checkbox/checkbox.component';
import { InputComponent } from '../../../../../../shared/input/input.component';

@Component({
  selector: 'user-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditComponent implements OnInit {
  @ViewChild('username') usernameRef?: InputComponent;
  @ViewChild('name') nameRef?: InputComponent;
  @ViewChild('email') emailRef?: InputComponent;
  @ViewChild('isAdmin', { static: true }) isAdminRef?: CheckboxComponent;
  @ViewChild('password') passwordRef?: InputComponent;

  @Input() user?: UserDto;
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  @Output() onEdit: EventEmitter<UpdateUserDto> =
    new EventEmitter<UpdateUserDto>();
  constructor() {}

  ngOnInit(): void {
    if (this.isAdminRef) {
      this.isAdminRef.isChecked = this.user!.isAdmin;
    }
  }

  edit(): void {
    this.onEdit.emit({
      username: this.usernameRef!.value,
      name: this.nameRef!.value,
      email: this.emailRef!.value,
      isAdmin: this.isAdminRef!.isChecked,
      password: this.passwordRef!.value,
    });
  }

  close(): void {
    this.onClose.emit();
  }
}
