import { Component, Input, OnInit } from '@angular/core';
import { MemberDto } from '../../../../team.interface';

@Component({
  selector: 'team-member',
  templateUrl: './member.component.html',
  styleUrls: ['./member.component.scss'],
})
export class MemberComponent implements OnInit {
  @Input() member?: MemberDto;
  @Input() isUser?: boolean;

  constructor() {}

  ngOnInit(): void {}

  kick(): void {}
}
