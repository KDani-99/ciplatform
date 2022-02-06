import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { UserService } from '../../user.service';
import { UserDto } from '../../user.interface';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
})
export class UsersComponent implements OnInit {
  users?: UserDto[];

  showWindow: boolean = false;
  selectedUser?: UserDto;
  currentTemplate?: TemplateRef<any>;

  @ViewChild('editTemplate') editTemplate?: TemplateRef<any>;
  @ViewChild('resetPasswordTemplate') resetPasswordTemplate?: TemplateRef<any>;
  @ViewChild('deleteTemplate') deleteTemplate?: TemplateRef<any>;

  constructor(private readonly userService: UserService) {}

  ngOnInit(): void {
    this.userService.getUsers().subscribe({
      next: (users: UserDto[]) => {
        this.users = users;
      },
    });
  }

  onEdit(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.editTemplate);
  }

  onResetPasswordClick(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.resetPasswordTemplate);
  }

  onDeleteClick(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.deleteTemplate);
  }

  onDelete(): void {
    if (this.selectedUser) {
      this.userService.deleteUser(this.selectedUser.id).subscribe({
        next: () => {
          this.users = this.users?.filter(
            (user) => user.id !== this.selectedUser?.id,
          ); // removes the user = returns users with different id
          this.selectedUser = undefined;
        },
      });
    }
  }

  onResetPassword(): void {
    if (this.selectedUser) {
      this.userService.resetPassword(this.selectedUser.id);
    }
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
