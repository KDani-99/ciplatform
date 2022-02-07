import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserComponent } from './components/user/user.component';
import { SharedModule } from '../../../../shared/shared.module';
import { EditComponent } from './components/edit/edit.component';
import { DeleteComponent } from './components/delete/delete.component';

@NgModule({
  declarations: [UserComponent, EditComponent, DeleteComponent],
  exports: [UserComponent, EditComponent, DeleteComponent],
  imports: [CommonModule, SharedModule],
})
export class UsersModule {}
