import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'shared-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
})
export class InputComponent implements OnInit {
  @Input() value: string = '';

  @Input() type: 'text' | 'password' | 'email' = 'text';
  @Input() header: string = '';
  @Input() placeholder: string = '';
  @Input() maxLength?: number;

  constructor() {}

  ngOnInit(): void {}
}
