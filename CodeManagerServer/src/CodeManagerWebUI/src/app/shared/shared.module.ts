import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputComponent } from './input/input.component';
import { ButtonComponent } from './button/button.component';
import { PageComponent } from './page/page.component';
import { FormsModule } from '@angular/forms';
import { PrimaryPageComponent } from './primary-page/primary-page.component';

@NgModule({
  declarations: [
    InputComponent,
    ButtonComponent,
    PageComponent,
    PrimaryPageComponent,
  ],
  exports: [
    InputComponent,
    ButtonComponent,
    PageComponent,
    PrimaryPageComponent,
  ],
  imports: [CommonModule, FormsModule],
})
export class SharedModule {}
