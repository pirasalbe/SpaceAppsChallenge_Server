-- phpMyAdmin SQL Dump
-- version 4.5.1
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Apr 30, 2017 at 01:25 AM
-- Server version: 10.1.19-MariaDB
-- PHP Version: 5.6.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `spaceapp`
--

-- --------------------------------------------------------

--
-- Table structure for table `details`
--

CREATE TABLE `details` (
  `idDetail` int(11) NOT NULL,
  `damage` text,
  `solution` text,
  `quality_grade` varchar(50) DEFAULT NULL,
  `identifications_count` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `details`
--

INSERT INTO `details` (`idDetail`, `damage`, `solution`, `quality_grade`, `identifications_count`) VALUES
(5391840, NULL, NULL, 'research', 2),
(5411418, NULL, NULL, 'research', 2),
(5411648, NULL, NULL, 'research', 2),
(5535838, NULL, NULL, 'research', 2),
(5539031, NULL, NULL, 'research', 2),
(5539032, NULL, NULL, 'research', 2),
(5582733, NULL, NULL, 'research', 6),
(5869860, NULL, NULL, 'casual', 1),
(5889749, NULL, NULL, 'research', 2),
(5890501, NULL, NULL, 'casual', 1);

-- --------------------------------------------------------

--
-- Table structure for table `reports`
--

CREATE TABLE `reports` (
  `idReport` int(11) NOT NULL,
  `locationX` float NOT NULL,
  `locationY` float NOT NULL,
  `timestamp` datetime NOT NULL,
  `trust` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `idTaxon` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `reports`
--

INSERT INTO `reports` (`idReport`, `locationX`, `locationY`, `timestamp`, `trust`, `email`, `idTaxon`) VALUES
(5391840, 41.4054, -87.3415, '2016-06-14 22:54:34', 5, 'prova@prova.com', 106653),
(5411418, 41.6527, -69.9664, '2016-06-22 19:22:00', 5, 'prova@prova.com', 3845),
(5411648, 41.628, -69.883, '2016-06-01 20:32:00', 5, 'prova@prova.com', 3845),
(5535838, 41.7867, -86.8925, '2016-06-11 18:52:38', 5, 'prova@prova.com', 52651),
(5539031, 38.8089, -77.1066, '2016-06-15 21:40:00', 5, 'prova@prova.com', 128487),
(5539032, 38.95, -77.1749, '2016-06-15 23:01:00', 5, 'prova@prova.com', 58657),
(5582733, 39.9458, -74.0695, '2016-06-19 21:24:10', 5, 'prova@prova.com', 50001),
(5869860, -1.13478, 10.1647, '2016-06-15 14:46:15', 5, 'prova@prova.com', 194090),
(5889749, 17.6276, -88.0658, '2016-06-21 16:16:00', 5, 'prova@prova.com', 15018),
(5890501, 17.7642, -88.6522, '2016-06-22 16:18:00', 5, 'prova@prova.com', 41673);

-- --------------------------------------------------------

--
-- Table structure for table `taxons`
--

CREATE TABLE `taxons` (
  `idTaxon` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `observationCount` int(11) NOT NULL,
  `wikipedia_summary` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `taxons`
--

INSERT INTO `taxons` (`idTaxon`, `name`, `observationCount`, `wikipedia_summary`) VALUES
(3845, 'Calidris canutus', 311, 'The <b>red knot</b> (<i>Calidris canutus</i>) (just <b>knot</b> in Europe) is a medium-sized shorebird which breeds in tundra and the Arctic Cordillera in the far north of Canada, Europe, and Russia. It is a large member of the <i>Calidris</i> sandpipers,'),
(15018, 'Melanoptila glabrirostris', 68, 'The <b>Black Catbird</b> (<i>Melanoptila glabrirostris</i>) is a songbird species in the monotypic genus <i><b>Melanoptila</b></i> of the family Mimidae. At 24 grams and 20 cm, it is the smallest mimid.'),
(41673, 'Nasua narica', 1090, 'The <b>white-nosed coati</b> (<i>Nasua narica</i>) is a species of coati and a member of the family Procyonidae (raccoons and relatives). Local names include <i><b>pizote</b></i>, <i><b>antoon</b></i>, and <i><b>tejón</b></i>. The last, which mainly is us'),
(50001, 'Terrapene carolina carolina', 1152, 'The <b>Eastern box turtle</b> <i><b>(Terrapene carolina carolina)</b></i> is a subspecies within a group of hinge-shelled turtles, normally called box turtles. <i>T. c. carolina</i> is native to an eastern part of the United States.'),
(52651, 'Sarracenia purpurea', 617, '<i><b>Sarracenia purpurea</b></i>, commonly known as the <b>purple pitcher plant</b>, <b>northern pitcher plant</b>, or <b>side-saddle flower</b>, is a carnivorous plant in the family Sarraceniaceae. Its range includes almost the entire eastern seaboard o'),
(58657, 'Ameiurus natalis', 122, 'The <b>yellow bullhead</b> (<i>Ameiurus natalis</i>) is a species of bullhead catfish that is a ray-finned fish that lacks scales.'),
(106653, 'Nehalennia irene', 158, '<i><b>Nehalennia irene</b></i> is a species of damselfly in the family Coenagrionidae. This species is commonly known as the <b>sedge sprite</b>.'),
(128487, 'Channa argus', 22, 'The <b>northern snakehead</b> (<i>Channa argus</i>) is a type of snakehead fish native to China, Russia, North Korea, and South Korea. In the United States, the fish is considered to be a highly invasive species. In a well-known incident, several were fou'),
(194090, 'Mecistops cataphractus', 11, 'The <b>African slender-snouted crocodile</b> (<i><b>Mecistops cataphractus</b></i>) is a species of crocodile. Recent studies in DNA and morphology suggest that it may belong in its own genus, <i>Mecistops</i>.');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `email` varchar(50) NOT NULL,
  `password` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`email`, `password`) VALUES
('prova@prova.com', '6258a5e0eb772911d4f92be5b5db0e14511edbe01d1d0ddd1d5a2cb9db9a56ba');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `details`
--
ALTER TABLE `details`
  ADD PRIMARY KEY (`idDetail`);

--
-- Indexes for table `reports`
--
ALTER TABLE `reports`
  ADD PRIMARY KEY (`idReport`),
  ADD KEY `email` (`email`),
  ADD KEY `idTaxon` (`idTaxon`);

--
-- Indexes for table `taxons`
--
ALTER TABLE `taxons`
  ADD PRIMARY KEY (`idTaxon`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `reports`
--
ALTER TABLE `reports`
  MODIFY `idReport` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5898371;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `details`
--
ALTER TABLE `details`
  ADD CONSTRAINT `details_ibfk_2` FOREIGN KEY (`idDetail`) REFERENCES `reports` (`idReport`);

--
-- Constraints for table `reports`
--
ALTER TABLE `reports`
  ADD CONSTRAINT `reports_ibfk_1` FOREIGN KEY (`email`) REFERENCES `users` (`email`),
  ADD CONSTRAINT `reports_ibfk_2` FOREIGN KEY (`idTaxon`) REFERENCES `taxons` (`idTaxon`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
