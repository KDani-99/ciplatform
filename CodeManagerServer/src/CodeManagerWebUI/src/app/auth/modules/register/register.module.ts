import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegisterComponent } from './register.component';
import { SharedModule } from '../../../shared/shared.module';

@NgModule({
  declarations: [RegisterComponent],
  imports: [CommonModule, SharedModule],
})
export class RegisterModule {}
