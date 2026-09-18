package com.myschool.app.data.remote

import com.myschool.app.domain.model.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {
    
    // Authentication
    @POST("auth/login")
    suspend fun login(@Body request: LoginRequest): Response<LoginResponse>
    
    // Schools (SystemAdmin only)
    @GET("schools")
    suspend fun getSchools(@Query("page") page: Int = 1, @Query("pageSize") pageSize: Int = 50): Response<List<School>>
    
    @GET("schools/{schoolId}")
    suspend fun getSchool(@Path("schoolId") schoolId: Int): Response<School>
    
    @POST("schools")
    suspend fun createSchool(@Body school: School): Response<School>
    
    @PUT("schools/{schoolId}")
    suspend fun updateSchool(@Path("schoolId") schoolId: Int, @Body school: School): Response<School>
    
    @DELETE("schools/{schoolId}")
    suspend fun deleteSchool(@Path("schoolId") schoolId: Int): Response<Unit>
    
    // Users
    @GET("users")
    suspend fun getUsers(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("search") search: String? = null,
        @Query("roleId") roleId: Int? = null
    ): Response<List<User>>
    
    @GET("users/{userId}")
    suspend fun getUser(@Path("userId") userId: Int): Response<User>
    
    @POST("users")
    suspend fun createUser(@Body user: User): Response<User>
    
    @PUT("users/{userId}")
    suspend fun updateUser(@Path("userId") userId: Int, @Body user: User): Response<User>
    
    @DELETE("users/{userId}")
    suspend fun deleteUser(@Path("userId") userId: Int): Response<Unit>
    
    // Students
    @GET("students")
    suspend fun getStudents(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("search") search: String? = null,
        @Query("classId") classId: Int? = null,
        @Query("academicYearId") academicYearId: Int? = null
    ): Response<List<Student>>
    
    @GET("students/{studentId}")
    suspend fun getStudent(@Path("studentId") studentId: Int): Response<Student>
    
    @POST("students")
    suspend fun createStudent(@Body student: Student): Response<Student>
    
    @PUT("students/{studentId}")
    suspend fun updateStudent(@Path("studentId") studentId: Int, @Body student: Student): Response<Student>
    
    @DELETE("students/{studentId}")
    suspend fun deleteStudent(@Path("studentId") studentId: Int): Response<Unit>
    
    @GET("students/{studentId}/balance")
    suspend fun getStudentBalance(@Path("studentId") studentId: Int): Response<Map<String, Any>>
    
    // Academic Years
    @GET("academic/years")
    suspend fun getAcademicYears(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50
    ): Response<List<AcademicYear>>
    
    @GET("academic/years/{yearId}")
    suspend fun getAcademicYear(@Path("yearId") yearId: Int): Response<AcademicYear>
    
    @POST("academic/years")
    suspend fun createAcademicYear(@Body year: AcademicYear): Response<AcademicYear>
    
    @PUT("academic/years/{yearId}")
    suspend fun updateAcademicYear(@Path("yearId") yearId: Int, @Body year: AcademicYear): Response<AcademicYear>
    
    @DELETE("academic/years/{yearId}")
    suspend fun deleteAcademicYear(@Path("yearId") yearId: Int): Response<Unit>
    
    // Terms
    @GET("academic/terms")
    suspend fun getTerms(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("academicYearId") academicYearId: Int? = null
    ): Response<List<AcademicTerm>>
    
    @GET("academic/terms/{termId}")
    suspend fun getTerm(@Path("termId") termId: Int): Response<AcademicTerm>
    
    @POST("academic/terms")
    suspend fun createTerm(@Body term: AcademicTerm): Response<AcademicTerm>
    
    @PUT("academic/terms/{termId}")
    suspend fun updateTerm(@Path("termId") termId: Int, @Body term: AcademicTerm): Response<AcademicTerm>
    
    @DELETE("academic/terms/{termId}")
    suspend fun deleteTerm(@Path("termId") termId: Int): Response<Unit>
    
    // Classes
    @GET("classes")
    suspend fun getClasses(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("academicYearId") academicYearId: Int? = null
    ): Response<List<ClassGroup>>
    
    @GET("classes/{classId}")
    suspend fun getClass(@Path("classId") classId: Int): Response<ClassGroup>
    
    @POST("classes")
    suspend fun createClass(@Body classGroup: ClassGroup): Response<ClassGroup>
    
    @PUT("classes/{classId}")
    suspend fun updateClass(@Path("classId") classId: Int, @Body classGroup: ClassGroup): Response<ClassGroup>
    
    @DELETE("classes/{classId}")
    suspend fun deleteClass(@Path("classId") classId: Int): Response<Unit>
    
    // Subjects
    @GET("subjects")
    suspend fun getSubjects(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("classId") classId: Int? = null
    ): Response<List<Subject>>
    
    @GET("subjects/{subjectId}")
    suspend fun getSubject(@Path("subjectId") subjectId: Int): Response<Subject>
    
    @POST("subjects")
    suspend fun createSubject(@Body subject: Subject): Response<Subject>
    
    @PUT("subjects/{subjectId}")
    suspend fun updateSubject(@Path("subjectId") subjectId: Int, @Body subject: Subject): Response<Subject>
    
    @DELETE("subjects/{subjectId}")
    suspend fun deleteSubject(@Path("subjectId") subjectId: Int): Response<Unit>
    
    // Exams
    @GET("exams")
    suspend fun getExams(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("academicYearId") academicYearId: Int? = null,
        @Query("termId") termId: Int? = null
    ): Response<List<Exam>>
    
    @GET("exams/{examId}")
    suspend fun getExam(@Path("examId") examId: Int): Response<Exam>
    
    @POST("exams")
    suspend fun createExam(@Body exam: Exam): Response<Exam>
    
    @PUT("exams/{examId}")
    suspend fun updateExam(@Path("examId") examId: Int, @Body exam: Exam): Response<Exam>
    
    @DELETE("exams/{examId}")
    suspend fun deleteExam(@Path("examId") examId: Int): Response<Unit>
    
    // Fees
    @GET("fees")
    suspend fun getFees(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("classId") classId: Int? = null,
        @Query("academicYearId") academicYearId: Int? = null
    ): Response<List<Fee>>
    
    @GET("fees/{feeId}")
    suspend fun getFee(@Path("feeId") feeId: Int): Response<Fee>
    
    @POST("fees")
    suspend fun createFee(@Body fee: Fee): Response<Fee>
    
    @PUT("fees/{feeId}")
    suspend fun updateFee(@Path("feeId") feeId: Int, @Body fee: Fee): Response<Fee>
    
    @DELETE("fees/{feeId}")
    suspend fun deleteFee(@Path("feeId") feeId: Int): Response<Unit>
    
    // Payments
    @GET("payments")
    suspend fun getPayments(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("studentId") studentId: Int? = null,
        @Query("startDate") startDate: String? = null,
        @Query("endDate") endDate: String? = null
    ): Response<List<FeePayment>>
    
    @POST("payments")
    suspend fun createPayment(@Body payment: FeePayment): Response<FeePayment>
    
    @GET("payments/{paymentId}")
    suspend fun getPayment(@Path("paymentId") paymentId: Int): Response<FeePayment>
    
    // Receipts
    @GET("receipts")
    suspend fun getReceipts(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("studentId") studentId: Int? = null
    ): Response<List<Receipt>>
    
    @GET("receipts/{receiptId}")
    suspend fun getReceipt(@Path("receiptId") receiptId: Int): Response<Receipt>
    
    @POST("receipts/reprint/{receiptId}")
    suspend fun reprintReceipt(@Path("receiptId") receiptId: Int): Response<Receipt>
    
    // Grades / Grading System
    @GET("grading-system")
    suspend fun getGradingSystems(@Query("level") level: String? = null): Response<List<Grade>>
    
    @GET("grading-system/{id}")
    suspend fun getGradingSystem(@Path("id") id: Int): Response<Grade>
    
    @POST("grading-system")
    suspend fun createGradingSystem(@Body grade: Grade): Response<Grade>
    
    @PUT("grading-system/{id}")
    suspend fun updateGradingSystem(@Path("id") id: Int, @Body grade: Grade): Response<Grade>
    
    @DELETE("grading-system/{id}")
    suspend fun deleteGradingSystem(@Path("id") id: Int): Response<Unit>
    
    // Teacher Assignments
    @GET("teacher-assignments")
    suspend fun getTeacherAssignments(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
        @Query("teacherId") teacherId: Int? = null,
        @Query("classId") classId: Int? = null
    ): Response<List<TeacherAssignment>>
    
    @POST("teacher-assignments")
    suspend fun createTeacherAssignment(@Body assignment: TeacherAssignment): Response<TeacherAssignment>
    
    @PUT("teacher-assignments/{assignmentId}")
    suspend fun updateTeacherAssignment(@Path("assignmentId") assignmentId: Int, @Body assignment: TeacherAssignment): Response<TeacherAssignment>
    
    @DELETE("teacher-assignments/{assignmentId}")
    suspend fun deleteTeacherAssignment(@Path("assignmentId") assignmentId: Int): Response<Unit>
    
    // Class Teachers
    @GET("class-teachers")
    suspend fun getClassTeachers(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50
    ): Response<List<ClassTeacher>>
    
    @GET("class-teachers/{id}")
    suspend fun getClassTeacher(@Path("id") id: Int): Response<ClassTeacher>
    
    @POST("class-teachers")
    suspend fun createClassTeacher(@Body classTeacher: ClassTeacher): Response<ClassTeacher>
    
    @PUT("class-teachers/{id}")
    suspend fun updateClassTeacher(@Path("id") id: Int, @Body classTeacher: ClassTeacher): Response<ClassTeacher>
    
    @DELETE("class-teachers/{id}")
    suspend fun deleteClassTeacher(@Path("id") id: Int): Response<Unit>
    
    // Student Promotion
    @GET("student-promotion/classes")
    suspend fun getPromotionClasses(): Response<List<PromotionClass>>
    
    @GET("student-promotion/students")
    suspend fun getPromotionStudents(@Query("classId") classId: Int?): Response<List<Student>>
    
    @POST("student-promotion/promote")
    suspend fun promoteStudents(@Body promotionRequest: PromotionRequest): Response<Map<String, Any>>
    
    @POST("student-promotion/bulk-change-class")
    suspend fun bulkChangeClass(@Body changeClassRequest: ChangeClassRequest): Response<Map<String, Any>>
    
    // Dashboard
    @GET("dashboard/stats")
    suspend fun getDashboardStats(): Response<DashboardStats>
    
    // Reports
    @GET("reports/student-list")
    suspend fun getStudentListReport(
        @Query("classId") classId: Int? = null,
        @Query("academicYearId") academicYearId: Int? = null
    ): Response<List<Student>>
    
    @GET("reports/fee-collection")
    suspend fun getFeeCollectionReport(
        @Query("startDate") startDate: String? = null,
        @Query("endDate") endDate: String? = null
    ): Response<Map<String, Any>>
    
    @GET("reports/outstanding-balances")
    suspend fun getOutstandingBalancesReport(@Query("classId") classId: Int? = null): Response<List<StudentBalance>>
}
