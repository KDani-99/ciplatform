import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserComponent } from './components/user/user.component';
import { SharedModule } from '../../shared/shared.module';
import { EditComponent } from './components/edit/edit.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { DeleteComponent } from './components/delete/delete.component';

@NgModule({
  declarations: [
    UserComponent,
    EditComponent,
    ResetPasswordComponent,
    DeleteComponent,
  ],
  exports: [
    UserComponent,
    EditComponent,
    ResetPasswordComponent,
    DeleteComponent,
  ],
  imports: [CommonModule, SharedModule],
})
export class UsersModule {}
