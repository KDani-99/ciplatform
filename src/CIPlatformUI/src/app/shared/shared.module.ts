import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputComponent } from './input/input.component';
import { ButtonComponent } from './button/button.component';
import { PageComponent } from './page/page.component';
import { FormsModule } from '@angular/forms';
import { PrimaryPageComponent } from './primary-page/primary-page.component';
import { TopbarComponent } from './page/components/topbar/topbar.component';
import { CheckboxComponent } from './checkbox/checkbox.component';

@NgModule({
  declarations: [
    InputComponent,
    ButtonComponent,
    PageComponent,
    PrimaryPageComponent,
    TopbarComponent,
    CheckboxComponent,
  ],
  exports: [
    InputComponent,
    ButtonComponent,
    PageComponent,
    PrimaryPageComponent,
    TopbarComponent,
    CheckboxComponent,
  ],
  imports: [CommonModule, FormsModule],
})
export class SharedModule {}
