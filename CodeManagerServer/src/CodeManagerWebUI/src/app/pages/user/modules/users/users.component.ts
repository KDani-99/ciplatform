import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { UserService } from '../../../../services/user/user.service';
import {
  UpdateUserDto,
  UserDto,
} from '../../../../services/user/user.interface';

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
  @ViewChild('deleteTemplate') deleteTemplate?: TemplateRef<any>;

  constructor(private readonly userService: UserService) {}

  ngOnInit(): void {
    this.userService.getUsers().subscribe({
      next: (users: UserDto[]) => {
        this.users = users;
      },
    });
  }

  onEditClick(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.editTemplate);
  }

  onDeleteClick(userDto: UserDto): void {
    this.selectedUser = userDto;
    this.toggleWindow(true, this.deleteTemplate);
  }

  onDelete(): void {
    if (this.selectedUser) {
      this.userService.deleteUser(this.selectedUser.id).subscribe(() => {
        this.users = this.users?.filter(
          (user) => user.id !== this.selectedUser?.id,
        ); // removes the user = returns users with different id
        this.selectedUser = undefined;
      });
    }
  }

  onEdit(updateUserDto: UpdateUserDto): void {
    if (this.selectedUser) {
      this.userService
        .updateUser(this.selectedUser.id, updateUserDto)
        .subscribe(() => {
          const index = this.users!.findIndex(
            (user) => user.id === this.selectedUser!.id,
          );
          this.users = [
            ...this.users!.slice(0, index),
            {
              ...this.users![index],
              ...updateUserDto,
            },
            ...this.users!.slice(index),
          ];
          this.selectedUser = undefined;
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
