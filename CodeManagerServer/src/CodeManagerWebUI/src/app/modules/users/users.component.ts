import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { UserDto } from './user.interface';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
})
export class UsersComponent implements OnInit {
  showWindow: boolean = false;
  selectedUser?: UserDto;
  currentTemplate?: TemplateRef<any>;

  @ViewChild('editTemplate') editTemplate?: TemplateRef<any>;
  @ViewChild('resetPasswordTemplate') resetPasswordTemplate?: TemplateRef<any>;
  @ViewChild('deleteTemplate') deleteTemplate?: TemplateRef<any>;

  constructor() {}

  ngOnInit(): void {}

  onEdit(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.editTemplate);
  }

  onResetPassword(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.resetPasswordTemplate);
  }

  onDelete(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.deleteTemplate);
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
      this.selectedUser = undefined;
    }
  }
}
