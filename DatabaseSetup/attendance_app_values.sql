-- Sample seed data for attendance_app
-- User enum values use the format e######, for example e0747028
-- Load order matters because of foreign key constraints:
-- users -> class/events -> studentclass/checkins

USE attendance_app;

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE `checkins`;
TRUNCATE TABLE `studentclass`;
TRUNCATE TABLE `events`;
TRUNCATE TABLE `class`;
TRUNCATE TABLE `users`;
SET FOREIGN_KEY_CHECKS = 1;

-- users
INSERT INTO `users`
(`userId`, `enum`, `fname`, `lname`, `email`, `password_hash`, `phoneNum`, `role`, `createdAt`)
VALUES
(1, 'e0747028', 'Jordan', 'Reed', 'jordan.reed@attendanceapp.edu', '$2y$10$examplehashadmin000000000000000000000000000000000000000000', '555-0101', 'admin', '2026-04-01 08:00:00'),
(2, 'e100001', 'Maya', 'Chen', 'maya.chen@attendanceapp.edu', '$2y$10$examplehashteacher10010000000000000000000000000000000000', '555-0102', 'teacher', '2026-04-01 08:05:00'),
(3, 'e100002', 'Daniel', 'Brooks', 'daniel.brooks@attendanceapp.edu', '$2y$10$examplehashteacher1002000000000000000000000000000000000', '555-0103', 'teacher', '2026-04-01 08:10:00'),
(4, 'e100003', 'Alicia', 'Patel', 'alicia.patel@attendanceapp.edu', '$2y$10$examplehashteacher1003000000000000000000000000000000000', '555-0104', 'teacher', '2026-04-01 08:15:00'),
(5, 'e200001', 'Noah', 'Williams', 'noah.williams@student.edu', '$2y$10$examplehashstudent2001000000000000000000000000000000000', '555-0201', 'student', '2026-04-02 09:00:00'),
(6, 'e200002', 'Emma', 'Garcia', 'emma.garcia@student.edu', '$2y$10$examplehashstudent20020000000000000000000000000000000000', '555-0202', 'student', '2026-04-02 09:05:00'),
(7, 'e200003', 'Liam', 'Smith', 'liam.smith@student.edu', '$2y$10$examplehashstudent20030000000000000000000000000000000000', '555-0203', 'student', '2026-04-02 09:10:00'),
(8, 'e200004', 'Olivia', 'Johnson', 'olivia.johnson@student.edu', '$2y$10$examplehashstudent200400000000000000000000000000000000', '555-0204', 'student', '2026-04-02 09:15:00'),
(9, 'e200005', 'Ethan', 'Brown', 'ethan.brown@student.edu', '$2y$10$examplehashstudent2005000000000000000000000000000000000', '555-0205', 'student', '2026-04-02 09:20:00'),
(10, 'e200006', 'Sophia', 'Davis', 'sophia.davis@student.edu', '$2y$10$examplehashstudent200600000000000000000000000000000000', '555-0206', 'student', '2026-04-02 09:25:00'),
(11, 'e200007', 'Mason', 'Miller', 'mason.miller@student.edu', '$2y$10$examplehashstudent200700000000000000000000000000000000', '555-0207', 'student', '2026-04-02 09:30:00'),
(12, 'e200008', 'Ava', 'Wilson', 'ava.wilson@student.edu', '$2y$10$examplehashstudent20080000000000000000000000000000000000', '555-0208', 'student', '2026-04-02 09:35:00'),
(13, 'e200009', 'Lucas', 'Moore', 'lucas.moore@student.edu', '$2y$10$examplehashstudent2009000000000000000000000000000000000', '555-0209', 'student', '2026-04-02 09:40:00'),
(14, 'e200010', 'Isabella', 'Taylor', 'isabella.taylor@student.edu', '$2y$10$examplehashstudent20100000000000000000000000000000000', '555-0210', 'student', '2026-04-02 09:45:00');

-- class
INSERT INTO `class`
(`classNo`, `className`, `teacher`, `createdAt`)
VALUES
(1, 'CS101 - Introduction to Programming', 2, '2026-04-03 10:00:00'),
(2, 'MATH201 - Applied Statistics', 3, '2026-04-03 10:05:00'),
(3, 'ENG150 - Academic Writing', 4, '2026-04-03 10:10:00'),
(4, 'CS220 - Database Systems', 2, '2026-04-03 10:15:00'),
(5, 'SCI110 - Environmental Science', 3, '2026-04-03 10:20:00');

-- events
INSERT INTO `events`
(`eventId`, `eventCode`, `eventName`, `eventTime`, `eventLocation`, `capacity`, `host`, `description`, `createdAt`)
VALUES
(1, 'CS101-APR20', 'CS101 Lecture: Variables and Data Types', '2026-04-20 09:00:00', 'Room 101', 40, 2, 'Regular lecture for CS101 students.', '2026-04-10 08:00:00'),
(2, 'MATH201-APR20', 'MATH201 Lab: Probability Review', '2026-04-20 11:00:00', 'Statistics Lab', 30, 3, 'Practice session covering probability rules.', '2026-04-10 08:10:00'),
(3, 'ENG150-APR21', 'ENG150 Workshop: Thesis Statements', '2026-04-21 10:00:00', 'Writing Center', 25, 4, 'Workshop for improving thesis statements.', '2026-04-10 08:20:00'),
(4, 'CS220-APR22', 'CS220 Lecture: SQL Joins', '2026-04-22 13:00:00', 'Room 204', 35, 2, 'Database lecture focused on inner and outer joins.', '2026-04-10 08:30:00'),
(5, 'SCI110-APR23', 'SCI110 Field Briefing', '2026-04-23 14:00:00', 'Science Hall', 45, 3, 'Briefing before the environmental field activity.', '2026-04-10 08:40:00'),
(6, 'ORIENT-APR24', 'Student Attendance App Orientation', '2026-04-24 15:00:00', 'Auditorium', 100, 1, 'General orientation on how to use the attendance system.', '2026-04-10 08:50:00');

-- studentclass
INSERT INTO `studentclass`
(`userId`, `classNo`)
VALUES
(5, 1), (5, 4),
(6, 1), (6, 2),
(7, 1), (7, 3),
(8, 2), (8, 3),
(9, 2), (9, 5),
(10, 3), (10, 5),
(11, 4), (11, 5),
(12, 1), (12, 4),
(13, 2), (13, 4),
(14, 3), (14, 5);

-- checkins
INSERT INTO `checkins`
(`checkInId`, `eventId`, `userId`, `checkedInAt`)
VALUES
(1, 1, 5, '2026-04-20 08:55:00'),
(2, 1, 6, '2026-04-20 08:56:00'),
(3, 1, 7, '2026-04-20 08:58:00'),
(4, 1, 12, '2026-04-20 08:59:00'),
(5, 2, 6, '2026-04-20 10:52:00'),
(6, 2, 8, '2026-04-20 10:54:00'),
(7, 2, 9, '2026-04-20 10:57:00'),
(8, 2, 13, '2026-04-20 10:59:00'),
(9, 3, 7, '2026-04-21 09:51:00'),
(10, 3, 8, '2026-04-21 09:54:00'),
(11, 3, 10, '2026-04-21 09:56:00'),
(12, 3, 14, '2026-04-21 09:58:00'),
(13, 4, 5, '2026-04-22 12:52:00'),
(14, 4, 11, '2026-04-22 12:55:00'),
(15, 4, 12, '2026-04-22 12:57:00'),
(16, 4, 13, '2026-04-22 12:59:00'),
(17, 5, 9, '2026-04-23 13:50:00'),
(18, 5, 10, '2026-04-23 13:54:00'),
(19, 5, 11, '2026-04-23 13:56:00'),
(20, 5, 14, '2026-04-23 13:59:00'),
(21, 6, 5, '2026-04-24 14:45:00'),
(22, 6, 6, '2026-04-24 14:46:00'),
(23, 6, 7, '2026-04-24 14:47:00'),
(24, 6, 8, '2026-04-24 14:48:00'),
(25, 6, 9, '2026-04-24 14:49:00'),
(26, 6, 10, '2026-04-24 14:50:00'),
(27, 6, 11, '2026-04-24 14:51:00'),
(28, 6, 12, '2026-04-24 14:52:00'),
(29, 6, 13, '2026-04-24 14:53:00'),
(30, 6, 14, '2026-04-24 14:54:00');
