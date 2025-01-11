import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule,FormsModule, ButtonsModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit {
  memberService = inject(MembersService);
  genderList = [{value: 'male', display: 'Male'}, {value: 'female', display: 'Female'}];


  ngOnInit(): void {
    if(!this.memberService.paginatedResult())
      this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers();
  }

  resetFilters(){
    this.memberService.resetFilters();
    this.loadMembers();
  }

  pageChanged(event: any){
    if(event.page!==this.memberService.userParams().pageNumber){
      this.memberService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }
}
